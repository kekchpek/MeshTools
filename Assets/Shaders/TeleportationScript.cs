using UnityEngine;

namespace Shaders
{
    public class TeleportationScript : MonoBehaviour
    {

        public float delay;

        bool up = true;

        bool stop;
        float stopTime;

        float _curVal = 0;

        Renderer _rendere;


        void Start()
        {
            _rendere = GetComponent<Renderer>();
            _rendere.sharedMaterial = new Material(_rendere.sharedMaterial);
        }

        void Update()
        {
            delay -= Time.deltaTime;
            if (delay > 0)
                return;

            if (stop)
            {
                stopTime += Time.deltaTime;
                if (stopTime > 2 && up || stopTime > 3 && !up)
                {
                    if(!up)
                    {
                        _rendere.sharedMaterial.SetFloat("_ClipBorderMax", 1);
                        _rendere.sharedMaterial.SetFloat("_ClipBorderMin", 0);
                    }
                    up = !up;
                    stop = false;
                    stopTime = 0;
                }
                else
                {
                    return;
                }
            }

            float scale = 2f;
            _curVal += Time.deltaTime * scale;
            if (up)
            {
                _rendere.sharedMaterial.SetFloat("_ClipBorderMax", Mathf.Clamp(_curVal, 0, 1));
            }
            else
            {
                _rendere.sharedMaterial.SetFloat("_ClipBorderMin", Mathf.Clamp(_curVal, 0, 1));
            }
            if (_curVal > 1)
            {
                _curVal = 0;
                stop = true;
            }
        }
    }
}
