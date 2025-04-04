﻿using Godot;
using System;
using System.Collections.Generic;

namespace UTheCat.Jumpvalley.Core.Music
{
    /// <summary>
    /// MusicPlayer that handles the playback of <see cref="Playlist"/>s with music zones in mind.
    /// <br/>
    /// Music zone logic is handled like how it is in Eternal Towers of Hell.
    /// This means that whenever a player enters a music zone, the zone's playlist gets played.
    /// <br/>
    /// In order for music to play when the player is outside a music zone, a primary playlist must be set.
    /// This playlist will get played when outside of the music zones.
    /// <br/>
    /// <br/>
    /// It's important to note that MusicZonePlayer will only update <see cref="CurrentPlaylist"/> once per process frame for stability reasons
    /// (as long as the calling of MusicZonePlayer's _Process() function hasn't been disabled).
    /// </summary>
    public partial class MusicZonePlayer : MusicPlayer, IDisposable
    {
        /// <summary>
        /// The list of <see cref="MusicZone"/>s that can be played by this <see cref="MusicZonePlayer"/>
        /// </summary>
        public List<MusicZone> Zones { get; private set; } = new List<MusicZone>();

        /// <summary>
        /// 3D position in global-space coordinates that will be used to check whether or not we're currently inside a music zone
        /// </summary>
        public Vector3 GlobalPosition = Vector3.Zero;

        /// <summary>
        /// If this is set to a node, the node's global-space position will be used to check whether or not we're currently inside a music zone,
        /// instead of the global-space position specified at <see cref="GlobalPosition"/>
        /// </summary>
        public Node3D BindedNode = null;

        public MusicZonePlayer() : base() { }

        /// <summary>
        /// Deregisters a music zone from playback.
        /// </summary>
        /// <param name="zone"></param>
        public void Remove(MusicZone zone)
        {
            if (zone != null)
            {
                RemovePlaylist(zone);
                Zones.Remove(zone);
            }
        }

        /// <summary>
        /// Registers a MusicZone for playback by this music player
        /// </summary>
        /// <param name="zone">The MusicZone</param>
        public void Add(MusicZone zone)
        {
            if (!Zones.Contains(zone))
            {
                AddPlaylist(zone);
                Zones.Add(zone);
            }
        }

        protected override void RefreshPlayback()
        {
            if (IsPlaying)
            {
                Vector3 pos;
                if (BindedNode == null)
                {
                    pos = GlobalPosition;
                }
                else
                {
                    pos = BindedNode.GlobalPosition;
                }

                // Play a music zone that we're currently in,
                // or switch back to the primary playlist if we aren't in any
                MusicZone selectedZone = null;
                for (int i = 0; i < Zones.Count; i++)
                {
                    MusicZone zone = Zones[i];

                    if (zone.IsPointInZone(pos))
                    {
                        selectedZone = zone;
                        break;
                    }
                }

                // check, just in case
                if (IsPlaying)
                {
                    if (selectedZone == null)
                    {
                        CurrentPlaylist = PrimaryPlaylist;
                    }
                    else
                    {
                        CurrentPlaylist = selectedZone;
                    }
                }
                else
                {
                    CurrentPlaylist = null;
                }
            }
            else
            {
                CurrentPlaylist = null;
            }
        }

        public override void _Process(double delta)
        {
            RefreshPlayback();

            base._Process(delta);
        }

        public new void Dispose()
        {
            SetProcess(false);

            // might as well clear the list of zones since you can't access the list after the object gets disposed anyways
            Zones.Clear();

            base.Dispose();
        }
    }
}
