using Ilumisoft.RadarSystem.UI;
using UnityEngine;

namespace Ilumisoft.RadarSystem
{
    /// <summary>
    /// Abstract base class for all locatables
    /// </summary>
    public abstract class LocatableComponent : MonoBehaviour
    {
        // Property untuk menentukan apakah locatable akan tetap terlihat atau disembunyikan saat di luar radius radar
        public abstract bool ClampOnRadar { get; set; }

        // Method yang dipanggil ketika objek diaktifkan
        protected virtual void OnEnable()
        {
            // Daftarkan locatable pada LocatableManager
            LocatableManager.Register(this);
        }

        // Method yang dipanggil ketika objek dinonaktifkan
        protected virtual void OnDisable()
        {
            // Hapus locatable dari LocatableManager
            LocatableManager.Unregister(this);
        }

        // Method abstrak untuk membuat ikon locatable
        public abstract LocatableIconComponent CreateIcon();
    }
}
