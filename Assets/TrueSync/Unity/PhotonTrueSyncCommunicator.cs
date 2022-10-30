using System;

namespace TrueSync {

    /**
     *  @brief Truesync's {@link ICommunicator} implementation based on PUN. 
     **/
    public class PhotonTrueSyncCommunicator : ICommunicator {

       //private LoadBalancingPeer loadBalancingPeer;

        //private static PhotonNetwork.EventCallback lastEventCallback;

        /**
         *  @brief Instantiates a new PhotonTrueSyncCommunicator based on a Photon's LoadbalancingPeer. 
         *  
         *  @param loadBalancingPeer Instance of a Photon's LoadbalancingPeer.
         **/
        internal PhotonTrueSyncCommunicator() {

        }

        public int RoundTripTime() {
            return -1;
        }

        public void OpRaiseEvent(byte eventCode, object message, bool reliable, int[] toPlayers) {

        }

        public void AddEventListener(OnEventReceived onEventReceived) {

        }

    }

}
