using System;
using System.Collections.Generic;
using _Game.Core.Services.IAP;

namespace _Game.Core.UserState._State
{
    public class PurchaseDataState : IPurchaseDataStateReadonly
    {
        public List<BoughtIAP> BoudhtIAPs;

        public event Action Changed;

        List<BoughtIAP> IPurchaseDataStateReadonly.BoughtIAPs => BoudhtIAPs;

        public void AddPurchase(string id)
        {
            BoughtIAP boughtIap = Product(id);

            if (boughtIap != null)
            {
                boughtIap.Count++;
            }
            else
            {
                BoudhtIAPs.Add(new BoughtIAP{IAPId = id, Count = 1});
            }
            
            Changed?.Invoke();
        }

        private BoughtIAP Product(string id)
        {
            return BoudhtIAPs.Find(x => x.IAPId == id);
        }
    }
}