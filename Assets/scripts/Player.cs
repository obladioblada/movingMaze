using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player {
   
   public GameObject playerGameObject;
   public string name;
   public int number;
   private int deviceId;
   private bool isActive;
   private string color;
   public GameObject playerLabel;
   public GameObject playerImage;
   public GameObject playerScoreLabel;
   public Vector2 initialPosition;
   public List<int> deviceIDs = new List<int>();
   public bool isMaster;

   public Stack<Card> cards;

   public Player(GameObject player, string name, int number, int deviceId, bool isActive, string color, bool isMaster = false) {
      playerGameObject = player;
      this.name = name;
      this.number = number;
      this.deviceId = deviceId;
      this.isActive = isActive;
      this.color = color;
      deviceIDs.Add(deviceId);
      this.isMaster = isMaster;

   }

   public override string ToString() {
      return "name: " + name + " number: " + number + " deviceId: " + deviceId + " color: " + color + " isActive: " + isActive + " activeCard: " + cards.Peek().id;
   }
}
