using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player {
   
   public GameObject playerGameObject;
   public string name;
   private int number;
   private int deviceId;
   private bool isActive;
   private string color;
   public Text playerLabel;
   public GameObject playerImage;
   public Text playerScoreLabel;

   public Stack<Card> cards;

   public Player(GameObject player, string name, int number, int deviceId, bool isActive, string color) {
      this.playerGameObject = player;
      this.name = name;
      this.number = number;
      this.deviceId = deviceId;
      this.isActive = isActive;
      this.color = color;
   }
}
