# Adventure-of-DonQuijo
 브릿지 2025-1 프로젝트 돈키호의 모험

이하 문서는 충돌 로직 구현의 이해를 돕기 위한 설명을 포함하고 있다.
문서의 설명은 아래 그림의 용어들을 기반으로 한다.

                           ________________________   <- 높이 h_box
                         / :                      /|                             화면상의 위치 (x, y) = (a, b + c)
                       /   :                    /  |                ___________  3차원 공간에서의 위치 (a, b, c)
                     /     :                  /    |               /            \ 
                   /       :                 /     |    Facing    |             |  Opposite
                 /_________:______________ /       |     <----    |             |     ---->
________________ | _ _ _ _ : _ _ _ _ _ _  | _ _ _  |______________|    Player   |________________
                 |        /               |        /              |             |               / |
                 |      /        Ground   |      /     높이    :::|             |:::          /   |
                 |    /          Layer    |    /    h_ground  :::: \___________/:::::       /     | ______________
                 |  /         Collider    |  /                 :::::::::::::::::::::      /      /        /| Vertical
________________ |/______________________ |/___________________________________________ /      /         /
|                                                                                       |    /  <------/------> Horizontal    
|                                                                                       |  /          /
|_______________________________________________________________________________________|/__________|/__________
(지면의 높이는 h_ground, 박스의 높이는 h_box -> Player가 박스 위에 있을 때 c는 h_ground + h_box)

 위와 같은 상황이 있다고 가정하자. 화면 상의 Player의 위치를 (x, y), 3차원 공간에서 나타내고 싶은 플레이어의 위치를 (a, b, c)라고 한다면, (x, y) = (a, b + c)라는 공식이 성립한다. 플레이어 게임 오브젝트는 매 FixedUpdate마다 아래의 좌표들을 재설정한다.
* currentScreenPosition = (x, y) = (a, b + c)
* currentSpacePosition = (a, b, c)
* currentProjectedPosition = (a, b, h_ground)
* currentEntityHeight = c
* currentGroundHeight = h_ground
* facingObstacleHeight = h_box

Player와 적을 포함한 모든 Entity들은 StateMachine 방식으로 작동한다. StateMachine의 대부분의 로직은 FixedUpdate에서 이루어지며, 이는 FixedUpdate에서 계산되는 Detection값으로 State 전이가 이루어짐으로써 발생하는, Update와 FixedUpdate 간의 동기화 문제를 방지하기 위함이다. 각 Entity는 Core를 가지고 있으며, Core는 현재 4개의 CoreComponent를 갖는 것으로 계획되어 있다. 각 CoreComponent는 서로 다른 기능을 담당하며, 현재 CoreComponent에는 Detection, Movement, Combat, Stat이 존재한다.

Player의 isGrounded 변수는 현재 지면에 붙어있는지를 나타내며 이를 기준으로 중력이 적용된다. isGrounded를 계산하는 구체적인 조건은 Player의 현재 State에 따라 달라진다. 
조건 1. c <= h_ground + 0.005f(이하, epsilon)
조건 2. currentVelocity.y < epsilon
Ex) 현재 State가 inAirState인 경우, isGrounded = 조건 1 && 조건 2
     현재 State가 GroundedState인 경우, isGrounded = 조건 1
     조건 2가 생략된 것은 Grounded인 경우 vertical 방향의 이동을 위해 y축 속도를 조정하기 때문이다.
이는 현재 고쳐야할 것으로 예상되는데, KnockbackState, AttackState 등 모든 State마다 조건을 따로 처리하는 것은 예상치 못한 오류가 발생할 것으로 예측되기 때문이다.

현재 Player는 구현 상의 한계로 공중에 있을 경우 vertical 이동을 하지 못한다. 이를 구현하기 위해선 currentVelocity = planeVelocity + zAxisVelocity의 계산이 이루어져야하는데, 이를 어떻게 구현해야하는지 모르겠다.

Jump 구현은 그림자를 나타내는 Shadow 게임 오브젝트에 착지를 위한 Collider를 만듦으로써 이루어졌다. Collider가 없을 경우 isGrounded가 false에서 true로 넘어가는 FixedUpdate의 간격 사이에 중력이 적용되어 Player가 아래로 이동하기 때문이다. 그림자 게임 오브젝트는 Entity의 자식 오브젝트가 아닌 형제 오브젝트로 분리되어 있는데, 만일 자식 오브젝트로 편입시킬 경우 불분명한 이유로 착지시 Collider의 위치가 이동하면서 착지 위치가 크게 틀어지게 된다.

모든 Entity는 currentProjectedPosition을 기준으로 FixedUpdate에서 BoxCast를 사용한다. Collider를 사용하지 않는 이유는 아무리 Update와 FixedUpdate에서 Collider의 offset을 초기화한다고 해도 결국 완전히 y축 위치를 고정할 수 없기 때문이다. 이 때문에 플레이어가 점프시 충돌이 발생, Opposite 방향으로의 x축 이동이 발생하게 된다. 이에 따라 Player는 자체적인 충돌 매커니즘을 사용한다.

모든 Ground Layer 게임 오브젝트는 z축 좌표를 지면의 높이로 가지며 모든 충돌은 이를 기준으로 발생한다.(이는 정보를 담기 위한 스크립트를 따로 만드는 등 여러 방향으로 수정될 수 있다.) 만약 앞서 설명한, Player의 Collider 안에 Ground Layer를 갖는 오브젝트가 발견되었고, 해당 오브젝트의 z축 좌표(지면의 높이)가 플레이어의 높이(currentEntityHeight)인 z축 좌표보다 크다면 Player의 속도를 0으로 조정한다. 그러나 해당 방식은 FixedUpdate에서 작동하므로 Entity가 빠른 속도로 이동할 경우 충돌이 감지를 하지 못하므로 충돌 보정 매커니즘을 통해 이를 보완해줘야 한다. 충돌 보정 매커니즘은 Entity의 현재 높이가 currentProjectedPosition에서 발견한 지면의 높이보다 낮을때 발생한다. 이는 다시 말해 Entity가 물체 안으로 뚫고 들어간 경우를 말한다. 만약 해당 상황에서 Player가 이동 중인 경우, 그 속도의 반대 방향으로 OverlapBox를 하면서 지면의 높이와 Player의 높이가 같은 곳으로 위치를 조정한다. 그러나 만약 Player가 이동 중이지 않은 경우, 현재 위치로부터 가장 가까운 지면의 Collider의 바깥쪽으로 이동한다. 

현재 고려 사항은 아래와 같다.
1. 물체 위에 투영되는 그림자 처리
2. 다수의 낭비되는 Layer
3. Grounded 조건
4. 점프 후 착지시 높이에 따라 y축값 소폭 변동
5. 비직관적인 맵 설계
6. 그림자 게임 오브젝트를 Entity의 자식으로 편입하는 방법
7. 충돌 매커니즘을 적용해도 1프레임 동안 물체 안으로 들어가는 현상을 해결하지 못함
8. 멈추는 위치가 종종 다르다.
