using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeUniverseTransformSync : UniverseTransformSync
{
    public override void OnConnectionMade()
    {
        base.OnConnectionMade();

        // Setup synched pipe to update pipe system when it should
        Pipe pipe = GetComponent<Pipe>();
        Pipe synchedPipe = syncedTransform.GetComponent<Pipe>();

        if (synchedPipe != null)
            UpdateSyncedPipesSystem(pipe, synchedPipe);
    }

    void UpdateSyncedPipesSystem(Pipe pipe, Pipe synchedPipe)
    {
        pipe.onRotate += (sync)=> synchedPipe.ParentPipeSystem.UpdatePipesStates(synchedPipe);
        pipe.onRotateCompleted += () => synchedPipe.ParentPipeSystem.UpdatePipesStates();
    }
}
