using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace PlagueEngine.Pathfinder
{
    [Serializable()]
    public class PathfinderManager
    {
        [NonSerialized]
        public static PathfinderManager PM;
        private float _boxLength;

        public float BoxLength
        {
            get { return _boxLength; }
            set { _boxLength = value; }
        }
        private float _boxWidth;

        public float BoxWidth
        {
            get { return _boxWidth; }
            set { _boxWidth = value; }
        }
        private float _boxHeight;

        public float BoxHeight
        {
            get { return _boxHeight; }
            set { _boxHeight = value; }
        }
        private int _numberOfBoxesInLength;

        public int NumberOfBoxesInLength
        {
            get { return _numberOfBoxesInLength; }
            set { _numberOfBoxesInLength = value; }
        }
        private int _numberOfBoxesInWidth;

        public int NumberOfBoxesInWidth
        {
            get { return _numberOfBoxesInWidth; }
            set { _numberOfBoxesInWidth = value; }
        }
        private float _distanceBeetwenBoxes;

        public float DistanceBeetwenBoxes
        {
            get { return _distanceBeetwenBoxes; }
            set { _distanceBeetwenBoxes = value; }
        }
        private Vector3 _boxStartPosition;

        public Vector3 BoxStartPosition
        {
            get { return _boxStartPosition; }
            set { _boxStartPosition = value; }
        }
        private HashSet<Node> _blockedNodes;

        public HashSet<Node> BlockedNodes
        {
            get { return _blockedNodes; }
            set { _blockedNodes = value; }
        }
        public PathfinderManager()
        {
            BlockedNodes = new HashSet<Node>();
        }
    }
}
