using DeployableOwnerInformation.Component;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DeployableOwnerInformation.Extension
{
    public static class OwnerInformationExtension
    {
        public static OwnerInformation GetOwnerInformation(this CharacterBody body)
        {
            return body.master.gameObject.GetComponent<OwnerInformation>();
        }

        public static OwnerInformation GetOwnerInformation(this CharacterMaster master)
        {
            return master.gameObject.GetComponent<OwnerInformation>();
        }

        public static OwnerInformation GetOwnerInformation(this GameObject obj)
        {
            return obj.GetComponent<OwnerInformation>();
        }
    }
}
