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

Player와 적을 포함한 모든 Entity들은 StateMachine 방식으로 작동한다. StateMachine의 대부분의 로직은 FixedUpdate에서 이루어지며, 이는 FixedUpdate에서 계산되는 Detection값으로 State 전이가 이루어짐으로써 발생하는, Update와 FixedUpdate 간의 동기화 문제를 방지하기 위함이다. 각 Entity는 Core를 가지고 있으며, Core는 현재 4개의 CoreComponent를 갖는 것으로 계획되어 있다. 각 CoreComponent는 서로 다른 기능을 담당하며, 현재 계획상 CoreComponent에는 Detection, Movement, Combat, Stat이 존재한다.

Player는 Character 게임 오브젝트와 Entity 게임 오브젝트로 분류된다. 우리는 십자키를 사용하여 Entity 게임 오브젝트를 조종하게 되며, 스페이스바를 누름으로써 Character 게임 오브젝트에 z축 방향으로의 속력을 가하게 된다. 이를 통해 기존의 방식에서 완전히 벗어나 공중에서도 자유롭게 이동이 가능하며, Collider를 통해 좀 더 섬세한 충돌을 다룰 수 있게 되었다.

현재 고려 사항은 아래와 같다.
1. 비직관적인 맵 설계
2. 멈추는 위치가 종종 다르다.
