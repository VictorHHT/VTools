using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTLayerMask
    {
        /// <summary>
        /// Get the index of a given layer mask.
        /// <para>For example the built in layer mask 'Default' has index 0, and 'UI' has index 5.</para>
        /// </summary>
        /// <param name="layerMaskName"></param>
        /// <returns>The index of the specified layer mask</returns>
        public static int GetLayerMaskIndex(string layerMaskName)
        {
            return LayerMask.NameToLayer(layerMaskName);
        }

        /// <summary>
        /// Create a layer mask value based on the provided layer names.
        /// Unity internally manages layers using a bitmask approach. Each layer corresponds to a bit position
        /// in an integer value. The layer mask is then created by setting the corresponding bits based on the 
        /// provided layer names.
        /// <para>
        /// Which means the value of a layer mask is a multiple of 2 (eg. 2^7 = 128)
        /// </para>
        /// </summary>
        /// <param name="layerMaskName">An array of layer mask names.</param>
        /// <returns>The layer mask value representing the specified layers.</returns>
        public static int GetLayerMaskValue(params string[] layerMaskName)
        {
            return LayerMask.GetMask(layerMaskName);
        }

        /// <summary>
        /// Get the name of the layer mask based on the given layer mask index
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <returns>The layer mask name of the specified layer index</returns>
        public static string GetLayerMaskName(int layerIndex)
        {
            return LayerMask.LayerToName(layerIndex);
        }

        /// <summary>
        /// Adds a layer mask to the existing layer mask.
        /// </summary>
        /// <param name="layerMask">The layer mask to add to.</param>
        /// <param name="layerMasksToAdd">The layer masks to add.</param>
        /// <returns>The combined layer mask.</returns>
        public static LayerMask AddLayerMask(LayerMask layerMask, params LayerMask[] layerMasksToAdd)
        {
            foreach (var mask in layerMasksToAdd)
            {
                layerMask |= mask;
            }

            return layerMask;
        }

        /// <summary>
        /// Removes a layer mask from the existing layer mask.
        /// </summary>
        /// <param name="layerMask">The layer mask to remove from.</param>
        /// <param name="layerMasksToRemove">The layer masks to remove.</param>
        /// <returns>The resulting layer mask after removal.</returns>
        public static LayerMask RemoveLayerMask(LayerMask layerMask, params LayerMask[] layerMasksToRemove)
        {
            foreach (var mask in layerMasksToRemove)
            {
                layerMask &= ~mask;
            }

            return layerMask;
        }
    }
}