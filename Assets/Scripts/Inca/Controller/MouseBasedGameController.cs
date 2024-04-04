using UnityEngine;
using UnityEngine.UI;

public class MouseBasedGameController : GameController
{
    [SerializeField]
    private Transform ballTransfrom;
    [SerializeField]
    private Transform hitBallTransfrom;

    Vector3 dir = Vector3.zero;
    float prevScale = 0;
    float startScale = 0;
    float targetScale = 1;
    float timer = 0;

    private void Update()
    {
        print(Input.mousePosition);
        ballTransfrom.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 10f));

        dir = ballTransfrom.position - Camera.main.transform.position;
        dir.Normalize();

        Ray ray = new Ray(Camera.main.transform.position, dir);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100000, targetLayermask))
        {
            rectTransform.position = Camera.main.WorldToScreenPoint(hitInfo.point);
            hitBallTransfrom.position = hitInfo.point;
            float dist = Vector3.Distance(Camera.main.transform.position, hitInfo.point);
            targetScale = Mathf.Lerp(0.1f, 2.4f, 5f / dist);

            if (prevScale == 1)
            {
                startScale = 1;
                timer = 0;
            }
            image.material = materialRed;
        }
        else
        {
            rectTransform.position = Input.mousePosition;
            targetScale = 1;
            if (prevScale < 0.95)
            {
                startScale = prevScale;
                timer = 0;
            }
            image.material = materialGreen;
        }

        timer += Time.deltaTime;
        rectTransform.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, sizeChangingCurve.Evaluate(timer / 0.5f));
        prevScale = rectTransform.localScale.x;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (dir * 100));
    }

}
