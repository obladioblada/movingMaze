using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {
   public readonly int id;
   public GameObject cardGO;

   public Card(int id) {
      this.id = id;
   }

   public Card(int id, GameObject cardGO) {
      this.id = id;
      this.cardGO = cardGO;
   }
}
