using Godot;
using System;

using UTheCat.Jumpvalley.Core.Players.Camera;
using UTheCat.Jumpvalley.Core.Players.Movement;

namespace JumpvalleyApp.Players.Movement
{
    /// <summary>
    /// For an instance of <see cref="BaseMover"/>, this sets <see cref="BaseMover.IsFastTurnEnabled"/> to true whenever a camera is in first person or when fast-turn has been toggled on by the user.
    /// The <see cref="BaseMover"/>'s <see cref="BaseMover.IsFastTurnEnabled"/> is otherwise set to false.
    /// </summary>
    public partial class FastTurnControl : Node, IDisposable
    {
        private static readonly string INPUT_MAP_FAST_TURN = "character_fast_turn";

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
        /// If the camera's ZoomOutDistance property is set to 0, <see cref="BaseMover.IsFastTurnEnabled"/> is turned on for <see cref="Mover"/>.
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

        private bool _userEnabledFastTurn;

        /// <summary>
        /// Whether or not the user has enabled fast-turn themselves.
        /// </summary>
        public bool UserEnabledFastTurn
        {
            get => _userEnabledFastTurn;
            set
            {
                _userEnabledFastTurn = value;
                Update();
            }
        }

        /// <summary>
        /// The number of meters to shift the camera to the right by whenever fast-turn is enabled
        /// </summary>
        public float CameraFastTurnOffset = 0f;

        /// <summary>
        /// Creates an instance of <see cref="FastTurnControl"/>
        /// </summary>
        /// <param name="mover">The mover to associate with</param>
        /// <param name="camera">The camera to associate with</param>
        public FastTurnControl(BaseMover mover, BaseCamera camera)
        {
            UserEnabledFastTurn = false;
            Mover = mover;
            Camera = camera;
        }

        /// <summary>
        /// Updates/refreshes the <see cref="BaseMover.IsFastTurnEnabled"/> property of <see cref="Mover"/>
        /// </summary>
        public void Update()
        {
            if (Mover != null && Camera != null)
            {
                bool shouldFastTurn = UserEnabledFastTurn || Camera.ZoomOutDistance <= 0;
                Mover.IsFastTurnEnabled = shouldFastTurn;

                if (shouldFastTurn)
                {
                    Camera.RightOffset = CameraFastTurnOffset;
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
            if (Input.IsActionJustPressed(INPUT_MAP_FAST_TURN))
            {
                UserEnabledFastTurn = !UserEnabledFastTurn;
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
