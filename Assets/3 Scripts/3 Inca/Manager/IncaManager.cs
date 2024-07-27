using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inca
{
    public abstract class IncaManager : MonoBehaviour
    {
        /// <summary>
        /// This Method is called by IncaMainManager when Start
        /// </summary>
        public abstract void Init();
    }
}
