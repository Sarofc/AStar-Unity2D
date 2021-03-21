using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saro.AStar
{
    public enum EPathProcessResult
    {
        Success,
        Error_Path_Not_Found,
        Error_Grid_NotFound,
        Error_Start_NotWalkable,
        Error_End_NotWalkable,
        Error_Start_IsEnd,
        Error_Path_Too_Long,
    }
}
