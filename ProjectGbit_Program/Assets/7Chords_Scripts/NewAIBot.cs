using UnityEngine;

public class AIPathfinding : MonoBehaviour
{
    public Transform target; // 目标位置
    public float moveSpeed = 5f; // 移动速度
    public float avoidanceRange = 3f; // 避障范围
    public LayerMask obstacleLayer; // 障碍物的图层

    private void Update()
    {
        // 计算朝向目标的方向
        Vector3 targetDirection = target.position - transform.position;
        targetDirection.y = 0f; // 保持在水平面上
        transform.rotation = Quaternion.LookRotation(targetDirection);

        // 射线检测障碍物
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, avoidanceRange, obstacleLayer))
        {
            // 如果检测到障碍物，计算避让方向
            Vector3 avoidDirection = Vector3.Cross(Vector3.up, hit.normal);
            Vector3 newDirection = targetDirection + avoidDirection * 10f; // 通过交叉乘积计算新方向
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else
        {
            // 没有检测到障碍物，直接向前移动
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }
}
