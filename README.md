# AStar-Unity2D

一个简易的2d寻路方案，基于网格。

## Getting Started

1. 编辑场景，为障碍物设置Layer，并添加BoxCollider2d组件。
2. 创建一个空物体，添加Maps.cs脚本，点击CreateMapData按钮。
3. 再创建一个空物体，添加PathRequestManager.cs脚本，将Maps添加进去。
4. 创建Agent物体，添加Agent.cs脚本，修改Key_provider对应PathRequestManager中的Key;添加Target.cs脚本，指定目标物体，运行。

详情见Example1

## Reference

<https://paul.pub/a-star-algorithm/>

<https://github.com/Epicguru/ThreadedPathfinding>

<https://github.com/qiao/PathFinding.js>

<https://www.youtube.com/watch?v=-L-WgKMFuhE&t=180s>

<https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp>
