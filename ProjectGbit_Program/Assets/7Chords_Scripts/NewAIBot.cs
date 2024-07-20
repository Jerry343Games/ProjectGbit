using UnityEngine;

public class AIPathfinding : MonoBehaviour
{
    public Transform target; // Ŀ��λ��
    public float moveSpeed = 5f; // �ƶ��ٶ�
    public float avoidanceRange = 3f; // ���Ϸ�Χ
    public LayerMask obstacleLayer; // �ϰ����ͼ��

    private void Update()
    {
        // ���㳯��Ŀ��ķ���
        Vector3 targetDirection = target.position - transform.position;
        targetDirection.y = 0f; // ������ˮƽ����
        transform.rotation = Quaternion.LookRotation(targetDirection);

        // ���߼���ϰ���
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, avoidanceRange, obstacleLayer))
        {
            // �����⵽�ϰ��������÷���
            Vector3 avoidDirection = Vector3.Cross(Vector3.up, hit.normal);
            Vector3 newDirection = targetDirection + avoidDirection * 10f; // ͨ������˻������·���
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else
        {
            // û�м�⵽�ϰ��ֱ����ǰ�ƶ�
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }
}
