using Godot;
using System;

using Jumpvalley.Players.Camera;
using Jumpvalley.Players.Movement;

namespace JumpvalleyApp.Players.Movement
{
    /// <summary>
    /// For an instance of <see cref="BaseMover"/>, this sets <see cref="BaseMover.IsRotationLocked"/> to true whenever a camera is in first person or when Shift-Lock has been toggled on by the user.
    /// The <see cref="BaseMover"/>'s <see cref="BaseMover.IsRotationLocked"/> is otherwise set to false.
    /// </summary>
    public partial class RotationLockControl : Node, IDisposable
    {
        private static readonly string INPUT_MAP_SHIFT_LOCK = "character_shift_lock";

        private BaseMover _mover;

        /// <summary>
        /// The <see cref="BaseMover"/> associated with this instance of <see cref="BaseMover"/>
        /// </summary>
        public BaseMover Mover
        {
            get => _mover;
            private set
            {
                _mover = value;
                Update();
            }
        }

        private BaseCamera _camera;

        /// <summary>
        /// The <see cref="BaseCamera"/> asssociated with this instance of <see cref="BaseMover"/>.
        /// If the camera's ZoomOutDistance property is set to 0, RotationLock is turned on for <see cref="Mover"/>.
        /// </summary>
        public BaseCamera Camera
        {
            get => _camera;
            set
            {
                // Disconnect event handler from old Camera value
                DisconnectZoomOutDistanceChanged();

                _camera = value;
                Update();
                ConnectZoomOutDistanceChanged();
            }
        }

        private bool _userEnabledShiftLock;

        /// <summary>
        /// Whether or not the user has enabled shift lock themselves.
        /// </summary>
        public bool UserEnabledShiftLock
        {
            get => _userEnabledShiftLock;
            set
            {
                _userEnabledShiftLock = value;
                Update();
            }
        }

        /// <summary>
        /// The number of meters to shift the camera to the right by whenever rotation lock is enabled
        /// </summary>
        public float CameraRotationLockOffset = 1f;

        /// <summary>
        /// Creates an instance of <see cref="RotationLockControl"/>
        /// </summary>
        /// <param name="mover">The mover to associate with</param>
        /// <param name="camera">The camera to associate with</param>
        public RotationLockControl(BaseMover mover, BaseCamera camera)
        {
            UserEnabledShiftLock = false;
            Mover = mover;
            Camera = camera;
        }

        /// <summary>
        /// Updates/refreshes the RotationLocked property of <see cref="Mover"/>
        /// </summary>
        public void Update()
        {
            if (Mover != null && Camera != null)
            {
                bool isRotationLocked = UserEnabledShiftLock || Camera.ZoomOutDistance <= 0;
                Mover.IsRotationLocked = isRotationLocked;

                if (isRotationLocked)
                {
                    Camera.RightOffset = CameraRotationLockOffset;
                }
                else
                {
                    Camera.RightOffset = 0f;
                }
            }
        }

        public new void Dispose()
        {
            QueueFree();
            base.Dispose();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (Input.IsActionJustPressed(INPUT_MAP_SHIFT_LOCK))
            {
                UserEnabledShiftLock = !UserEnabledShiftLock;
            }

            base._Input(@event);
        }

        private void HandleZoomOutDistanceChanged(object sender, float _args)
        {
            Update();
        }

        private bool isZoomOutDistanceChangedConnected = false;

        private void ConnectZoomOutDistanceChanged()
        {
            if (!isZoomOutDistanceChangedConnected)
            {
                isZoomOutDistanceChangedConnected = true;

                if (_camera != null)
                {
                    _camera.ZoomOutDistanceChanged += HandleZoomOutDistanceChanged;
                }
            }
        }

        private void DisconnectZoomOutDistanceChanged()
        {
            if (isZoomOutDistanceChangedConnected)
            {
                if (_camera != null)
                {
                    _camera.ZoomOutDistanceChanged -= HandleZoomOutDistanceChanged;
                }
                isZoomOutDistanceChangedConnected = false;
            }
        }
    }
}
