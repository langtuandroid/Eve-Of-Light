using NeoSaveGames;
using NeoSaveGames.Serialization;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace NeoFPS
{
    [HelpURL("https://docs.neofps.com/manual/interactionref-mb-standardcarrysystem.html")]
    public class StandardCarrySystem : RigidbodyCarrySystem
	{
		private Carryable carryable = null;

		protected override bool CanCarryTarget(Rigidbody target)
		{
            if (!base.CanCarryTarget(target))
                return false;

            var c = target.GetComponent<Carryable>();
			return c != null && c.CanCarry();
		}

        protected override void OnObjectPickedUp()
        {
            base.OnObjectPickedUp();

            // Notify the carryable it's been picked up
            carryable = carryTarget.GetComponent<Carryable>();
            if (carryable != null)
                carryable.OnPickedUp(this);
        }

        protected override void OnObjectDropped()
        {
            base.OnObjectDropped();

            // Notify the carryable it's been dropped
            if (carryable != null)
                carryable.OnDropped(this);
            carryable = null;
        }

        protected override bool CanManipulate()
        {
            return base.CanManipulate() && carryable.manipulatable;
        }

        protected override Vector3 GetOffset()
        {
            return carryable.centerOffset;
        }

        public override void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
        {
            base.ReadProperties(reader, nsgo);

            if (didLoadFromSave && carryTarget != null)
                carryable = carryTarget.GetComponent<Carryable>();
        }
    }
}
