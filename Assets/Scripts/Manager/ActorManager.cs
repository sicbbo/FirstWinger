using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager
{
    private Dictionary<int, Actor> actors = new Dictionary<int, Actor>();

    public bool Regist(int _actorInstanceID, Actor _actor)
    {
        if (_actorInstanceID == 0)
        {
            Debug.LogError(string.Format("Regist Error! ActorInstanceID is not set! ActorInstanceID = {0}", _actorInstanceID));
            return false;
        }

        if (actors.ContainsKey(_actorInstanceID) == true)
        {
            if (_actor.GetInstanceID() != actors[_actorInstanceID].GetInstanceID())
            {
                Debug.LogError(string.Format("Regist Error! already exist! ActorInstanceID = {0}", _actorInstanceID));
                return false;
            }

            Debug.Log(string.Format("{0} is already registed!", _actorInstanceID));
            return true;
        }

        actors.Add(_actorInstanceID, _actor);
        Debug.Log(string.Format("Actor Regist id = {0}, actor = {1}", _actorInstanceID, _actor.name));
        return true;
    }

    public Actor GetActor(int _actorInstanceID)
    {
        if (actors.ContainsKey(_actorInstanceID) == false)
        {
            Debug.LogError(string.Format("GetActor Error! no exist! ActorInstanceID = {0}", _actorInstanceID));
            return null;
        }

        return actors[_actorInstanceID];
    }
}