using UnityEngine;

namespace Shaders.Scripts
{
    public class MandelbrotController : MonoBehaviour
    {

        private const string SCALE_ID = "_Scale";
        private const string CENTER_X_ID = "_CenterX";
        private const string CENTER_Y_ID = "_CenterY";

        private Material material;

        private float scale;
        private float x;
        private float y;

        [SerializeField] private float speed = 1;

        void Start()
        {
            material = GetComponent<Renderer>().material;
            scale = material.GetFloat(SCALE_ID);
            x = material.GetFloat(CENTER_X_ID);
            y = material.GetFloat(CENTER_Y_ID);
        }


        void Update()
        {
            if (Input.GetKey(KeyCode.E))
            {
                scale /= 1.1f;
                material.SetFloat(SCALE_ID, scale);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                scale *= 1.1f;
                material.SetFloat(SCALE_ID, scale);
            }
            if (Input.GetKey(KeyCode.W))
            {
                y += speed * scale;
                material.SetFloat(CENTER_Y_ID, y);
            }
            if (Input.GetKey(KeyCode.S))
            {
                y -= speed * scale;
                material.SetFloat(CENTER_Y_ID, y);
            }
            if (Input.GetKey(KeyCode.A))
            {
                x -= speed * scale;
                material.SetFloat(CENTER_X_ID, x);
            }
            if (Input.GetKey(KeyCode.D))
            {
                x += speed * scale;
                material.SetFloat(CENTER_X_ID, x);
            }
        }
    }
}
