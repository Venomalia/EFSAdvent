using AuroraLib.Core.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

namespace FSALib
{
    /// <summary>
    /// Represents a collection of actors in FSA game environment.
    /// </summary>
    public sealed class ActorList : ObservableCollection<Actor>, IBinaryObject
    {
        /// <inheritdoc cref="ObservableCollection{T}.ObservableCollection()"/>
        public ActorList()
        {
        }

        /// <inheritdoc cref="ObservableCollection{T}.ObservableCollection(IEnumerable{T})"/>
        public ActorList(IEnumerable<Actor> collection) : base(collection)
        {
        }

        /// <inheritdoc cref="ObservableCollection{T}.ObservableCollection(List{T})"/>
        public ActorList(List<Actor> list) : base(list)
        {
        }

        /// <inheritdoc/>
        protected override void InsertItem(int index, Actor item)
        {
            int sortIndex = GetInsertIndex(item);
            base.InsertItem(sortIndex, item);
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, Actor item)
        {
            base.SetItem(index, item);
            // sort the list and find the new index
            ((List<Actor>)Items).Sort();
            int sortIndex = ((List<Actor>)Items).BinarySearch(item);

            if (sortIndex != index)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, sortIndex, index));
            }
        }

        /// <inheritdoc/>
        protected override void MoveItem(int oldIndex, int newIndex)
            => Trace.WriteLine($"Moving items is not allowed, the {nameof(ActorList)} is sorted.");

        private int GetInsertIndex(Actor item)
        {
            int index = ((List<Actor>)Items).BinarySearch(item);

            if (index < 0)
            {
                index = ~index;
            }
            return index;
        }

        /// <inheritdoc cref="List{T}.BinarySearch(T)"/>
        public new int IndexOf(Actor item)
            => ((List<Actor>)Items).BinarySearch(item);

        /// <inheritdoc/>
        public void BinaryDeserialize(Stream source)
        {
            Clear();
            source.ReadCollection(Items, (int)source.Length / 11);
            Items.RemoveAt(Items.Count - 1); // Remove Null Actor
            ((List<Actor>)Items).Sort();
        }

        /// <inheritdoc/>
        public void BinarySerialize(Stream dest)
        {
            dest.WriteCollection(Items);
            dest.Write(Actor.Null);
        }

        /// <summary>
        /// Generates the folder path where actor-related binary files are stored for a given map index.
        /// </summary>
        /// <param name="path">The base directory path.</param>
        /// <param name="mapIndex">The index of the map.</param>
        /// <returns>The full folder path.</returns>
        public static string GetFolder(string path, int mapIndex)
            => Path.Combine(path, "bin", $"b{mapIndex:D3}");

        /// <summary>
        /// Generates the filename for an enemy actor list file based on the given map and room index.
        /// </summary>
        /// <param name="mapIndex">The index of the map.</param>
        /// <param name="roomIndex">The index of the room.</param>
        /// <returns>The formatted filename for the enemy actor list.</returns>
        public static string GetFileName(int mapIndex, int roomIndex)
            => $"d_enemy_map{mapIndex:D3}_{roomIndex:D2}.bin";

        /// <summary>
        /// Constructs the full file path for an enemy actor list file, combining the folder path and filename.
        /// </summary>
        /// <param name="path">The base directory path.</param>
        /// <param name="mapIndex">The index of the map.</param>
        /// <param name="roomIndex">The index of the room.</param>
        /// <returns>The full file path to the enemy actor list file.</returns>
        public static string GetFilePath(string path, int mapIndex, int roomIndex)
            => Path.Combine(GetFolder(path, mapIndex), GetFileName(mapIndex, roomIndex));
    }
}
