using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;
using MelonLoader;

namespace PW_Shield_Hero
{
    public class Shield : MonoBehaviour
    {
        public Shield(IntPtr ptr) : base(ptr) { }

        // Optional, only used in case you want to instantiate this class in the mono-side
        // Don't use this on MonoBehaviours / Components!
        public Shield() : base(ClassInjector.DerivedConstructorPointer<Shield>()) => ClassInjector.DerivedConstructorBody(this);

        public Transform parent;
        public Collider collider;

        public Vector3 angleOffset = new Vector3(45, 0, 0);

        public void Update()
        {
            this.transform.position = parent.position;
            this.transform.rotation = parent.rotation * Quaternion.Euler(angleOffset);

            
        }


        public void OnCollisionEnter(Collision col)
        {
            var projectile = col.transform.GetComponent<Projectile>();
            if (projectile != null)
            {
                MelonLogger.Msg("Projectile hit shield");

                
            }
        }
    }
}
