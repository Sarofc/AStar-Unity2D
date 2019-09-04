# AStar-Unity2D

一个简易的2d寻路方案，基于网格。

<img src = "https://github.com/Sarofc/AStar-Unity2D/blob/master/src/GIF.gif?raw=true" with = 30%>

## Getting Started

1. 编辑场景，为障碍物设置Layer，并添加BoxCollider2d组件。
2. 创建一个空物体，添加Maps.cs脚本，配置网格参数（grid size、node size、grid center等），CheckLayer选择layer（障碍物）， 点击CreateMapData按钮烘焙网格。
3. 再创建一个空物体，添加PathRequestManager.cs脚本，将Maps添加进去。
4. 创建Agent物体，添加Agent.cs脚本，修改Key_provider对应PathRequestManager中的Key;添加Target.cs脚本，指定目标物体，运行。

详情见Example1

## Screenshot

### 烘焙截图，左为烘焙，右为场景

<img src = "https://github.com/Sarofc/AStar-Unity2D/blob/master/src/%E6%89%B9%E6%B3%A8%202019-08-31%20193735.jpg?raw=true" with = 30%>

### 移动方式

1. Cut Corner

   <img src = "https://github.com/Sarofc/AStar-Unity2D/blob/master/src/cut corner.jpg?raw=true" with = 30%>
2. Not Cut Corner

   <img src = "https://github.com/Sarofc/AStar-Unity2D/blob/master/src/not cut corner.jpg?raw=true" with = 30%>
3. Never(4 direction)

   <img src = "https://github.com/Sarofc/AStar-Unity2D/blob/master/src/never.jpg?raw=true" with = 30%>

## Reference

<https://paul.pub/a-star-algorithm/>

<https://github.com/Epicguru/ThreadedPathfinding>

<https://github.com/qiao/PathFinding.js>

<https://www.youtube.com/watch?v=-L-WgKMFuhE&t=180s>

<https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp>
