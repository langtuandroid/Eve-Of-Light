using NeoSaveGames;
using NeoSaveGames.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoFPS.ModularFirearms
{
    [HelpURL("https://docs.neofps.com/manual/weaponsref-mb-modularfirearmdrop.html")]
	public class ModularFirearmDrop : FpsInventoryWieldableDrop, INeoSerializableComponent
	{
        [SerializeField, Tooltip("The ammo pickup for this weapon's magazine.")]
        private ModularFirearmAmmoPickup m_AmmoPickup = null;

        [SerializeField, Range(0.1f, 2f), Tooltip("The delay from dropping before the ammo pickup becomes active (prevents the dropper from instantly grabbing ammo)")]
        private float m_AmmoPickupDelay = 0.5f;

        private ModularFirearmPayload m_Payload = null;
        private Coroutine m_InitialisationCoroutine = null;

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (m_AmmoPickup == null)
                m_AmmoPickup = GetComponentInChildren<ModularFirearmAmmoPickup>();
        }
        #endif

        void Start()
        {
            if (m_InitialisationCoroutine == null)
                m_InitialisationCoroutine = StartCoroutine(Initialise(-1));
        }

        public override void Drop(IInventoryItem item, Vector3 position, Vector3 forward, Vector3 velocity)
        {
            base.Drop(item, position, forward, velocity);

            if (m_AmmoPickup != null)
                m_AmmoPickup.EnablePickup(false);

            var firearm = item.GetComponent<IModularFirearm>();
            if (firearm != null)
            {
                m_Payload = GetPayloadForFirearm(firearm);
                pickup.onPickedUp += OnFirearmPickedUp;

                // Sort out the ammo pickup
                if (m_AmmoPickup != null)
                {
                    if (m_InitialisationCoroutine != null)
                        StopCoroutine(m_InitialisationCoroutine);
                    m_InitialisationCoroutine = StartCoroutine(Initialise(m_Payload.magazineCount));
                }
            }
        }

        private void OnFirearmPickedUp(IInventory inventory, IInventoryItem item)
        {
            var firearm = item.GetComponent<IModularFirearm>();
            if (firearm != null && firearm is IModularFirearmPayloadReceiver payloadReceiver)
                payloadReceiver.SetStartupPayload(m_Payload);
        }

        private void OnAmmoPickedUp(ICharacter character, IPickup pickup)
        {
            var ammoPickup = pickup as ModularFirearmAmmoPickup;
            m_Payload.magazineCount = ammoPickup.quantity;
        }

        protected virtual ModularFirearmPayload GetPayloadForFirearm(IModularFirearm firearm)
        {
            var payload = new ModularFirearmPayload();
            payload.BuildFromFirearm(firearm);
            return payload;
        }

        IEnumerator Initialise(int ammoCount)
        {
            // Wait for the ammo pickup delay
            float timer = m_AmmoPickupDelay;
            while (pickup.item == null || timer > 0f)
            {
                yield return null;
                timer -= Time.deltaTime;
            }
            
            // Transfer the ammo count to the pickup
            if (ammoCount != -1)
            {
                if (m_AmmoPickup != null)
                {
                    m_AmmoPickup.quantity = ammoCount;
                    m_AmmoPickup.EnablePickup(true);
                    m_AmmoPickup.onPickupTriggered += OnAmmoPickedUp;
                }
                else
                {
                    var pickupFirearm = pickup.item.GetComponent<ModularFirearm>();
                    if (pickupFirearm != null)
                        pickupFirearm.reloader.startingMagazine = ammoCount;
                }
            }

            m_InitialisationCoroutine = null;
        }

        #region SAVE SYSTEM

        public void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
        {
            if (m_Payload != null)
            {
                writer.PushContext(SerializationContext.ObjectNeoSerialized, 0);
                m_Payload.WriteProperties(writer);
                writer.PopContext(SerializationContext.ObjectNeoSerialized);
            }
        }

        public void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
        {
            if (reader.PushContext(SerializationContext.ObjectNeoSerialized, 0))
            {
                try
                {
                    m_Payload = new ModularFirearmPayload();
                    m_Payload.ReadProperties(reader);

                    pickup.onPickedUp += OnFirearmPickedUp;

                    // Sort out the ammo pickup
                    if (m_AmmoPickup != null)
                    {
                        if (m_InitialisationCoroutine != null)
                            StopCoroutine(m_InitialisationCoroutine);
                        m_InitialisationCoroutine = StartCoroutine(Initialise(m_Payload.magazineCount));
                    }
                }
                finally
                {
                    reader.PopContext(SerializationContext.ObjectNeoSerialized, 0);
                }
            }
        }

        #endregion
    }
}