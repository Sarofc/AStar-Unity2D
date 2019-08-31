namespace Saro.AStar
{
    public enum PathProcessResult
    {
        Success,
        Cancelled,
        Error_Start_IsEnd,
        Error_Path_Too_Long,
        Error_Start_NotWalkable,
        Error_End_NotWalkable,
        Error_Path_Not_Found,
        Error_Grid_NotFound
    }
}