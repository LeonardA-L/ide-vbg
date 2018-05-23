using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class WallEffect : MonoBehaviour
    {
        public int parts = 4;
        public float partDiameter = 1;
        public float maxDistance = 10.0f;
        public GameObject wallPart;
        public Vector3 offset;
        private bool done = false;

        public void CreateWall()
        {
            if (done)
                return;
            done = true;

            transform.position += offset.x * transform.right
                                + (offset.y + 0.2f) * Vector3.up
                                + offset.z * transform.forward;

            int layerMask = 1 << 8;

            float distance = -1.0f;
            float altitude = 0.0f;
            bool[] partsValidations = new bool[parts];

            for (int i = 0; i < parts; i++)
            {
                RaycastHit hit;

                Vector3 position = transform.position;
                position += (i - Mathf.Floor(parts / 2.0f)) * partDiameter * transform.right + ((parts % 2 == 0) ? partDiameter / 2.0f * transform.right : Vector3.zero);

                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(position, transform.TransformDirection(Vector3.down), out hit, maxDistance, layerMask))
                {
                    //Debug.DrawRay(position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);

                    if (hit.distance > distance && hit.distance < maxDistance)
                    {
                        distance = hit.distance;
                        altitude = hit.point.y;
                    }

                    partsValidations[i] = hit.distance < maxDistance;
                }
            }
            
            if(distance == -1.0f)
            {
                Debug.Log("No target for wall");
                return;
            }

            GameObject wallContainer = new GameObject();
            wallContainer.name = "Wall";
            wallContainer.transform.position = transform.localPosition;
            wallContainer.transform.position = new Vector3(wallContainer.transform.position.x, altitude, wallContainer.transform.position.z);

            for (int i = 0; i < parts; i++)
            {
                if (!partsValidations[i])
                    continue;

                GameObject w = GameObject.Instantiate(wallPart, wallContainer.transform);
                w.transform.localPosition = Vector3.zero;
                w.transform.position += (i - Mathf.Floor(parts / 2.0f)) * new Vector3(partDiameter, 0, 0) + ((parts % 2 == 0) ? new Vector3(partDiameter / 2.0f, 0, 0) : Vector3.zero);
                Transform parentView = w.transform.Find("Wall_View");
                Transform view = parentView.transform.Find("Wall_0" + (i + 1));
                view.gameObject.SetActive(true);
            }
            wallContainer.transform.rotation = transform.rotation;
        }
    }
}