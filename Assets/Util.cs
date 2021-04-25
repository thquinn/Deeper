﻿using Assets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Coor = System.Tuple<int, int>;

namespace Assets {
    public static class Util {
        public static List<Coor> FindPath(Floor floor, Coor start, Coor end, Entity entity) {
            if (start.Equals(end)) {
                return new List<Coor>{ start };
            }
            // BFS.
            Dictionary<Coor, Coor> parents = new Dictionary<Coor, Coor>();
            Queue<Coor> queue = new Queue<Coor>();
            queue.Enqueue(start);
            while (queue.Count > 0 && !parents.ContainsKey(end)) {
                Coor currentCoor = queue.Dequeue();
                Tile currentTile = floor.tiles[currentCoor.Item1, currentCoor.Item2];
                List<Tile> neighbors = currentTile.GetNeighbors();
                neighbors.Shuffle();
                foreach (Tile neighbor in neighbors) {
                    Coor neighborCoor = new Coor(neighbor.x, neighbor.y);
                    if (parents.ContainsKey(neighborCoor)) {
                        continue;
                    }
                    if (!currentTile.floor.CanPassBetween(currentCoor, neighborCoor, entity)) {
                        continue;
                    }
                    parents[neighborCoor] = currentCoor;
                    queue.Enqueue(neighborCoor);
                }
            }
            if (!parents.ContainsKey(end)) {
                return null;
            }
            List<Coor> path = new List<Coor>();
            Coor current = end;
            path.Add(current);
            while (!current.Equals(start)) {
                current = parents[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }

        static Camera mainCamera;
        public static Collider GetMouseCollider(LayerMask layerMask) {
            if (mainCamera == null) mainCamera = Camera.main;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                return null;
            }
            return hit.collider;
        }

        // extension methods
        public static T[] Shuffle<T>(this T[] array) {
            int n = array.Length;
            for (int i = 0; i < n; i++) {
                int r = i + UnityEngine.Random.Range(0, n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
            return array;
        }
        public static List<T> Shuffle<T>(this List<T> list) {
            int n = list.Count;
            for (int i = 0; i < n; i++) {
                int r = i + UnityEngine.Random.Range(0, n - i);
                T t = list[r];
                list[r] = list[i];
                list[i] = t;
            }
            return list;
        }
    }
}
