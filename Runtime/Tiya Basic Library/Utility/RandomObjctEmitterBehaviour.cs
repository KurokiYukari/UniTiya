using UnityEngine;

namespace Sarachan.UniTiya.Utility
{
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Utility/Random GameObject Emitter")]
    class RandomObjctEmitterBehaviour : MonoBehaviour
    {
        [SerializeField] Transform _instantiatePositionOverride;
        [SerializeField] bool _synchronizeObjTag;
        [SerializeField] RandomObjctEmitter _randomObjs;

        public void InstantiatePrefab()
        {
            var instantiateTrans = _instantiatePositionOverride ? _instantiatePositionOverride : transform;
            var obj = _randomObjs.InstantiatePrefab(instantiateTrans.position, instantiateTrans.rotation);
            if (_synchronizeObjTag)
            {
                obj.SetChildrenLayerTo(gameObject.layer);
            }
        }
    }
}
