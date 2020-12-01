namespace Redis.FSharp

open StackExchange.Redis

type RedisConnectionBuilder =
    { ConnectionString : string }

[<RequireQualifiedAccess>]
module Redis =
    (* Connections*)
    let init =
        { ConnectionString = "" }
    
    let connString connString builder =
        { builder with ConnectionString = connString }

    let connect () builder =
        ConnectionMultiplexer.Connect builder.ConnectionString


    (* Database *)
    let db (multiplexer : ConnectionMultiplexer) =
        multiplexer.GetDatabase()
    
    (* Pub/Sub *)
    let subscribder (multiplexer : ConnectionMultiplexer) =
        multiplexer.GetSubscriber()
    
    let subscribe channel observable (sub : ISubscriber) =
        sub.Subscribe(channel, observable)
    
    let publish channel msg (sub : ISubscriber) =
        sub.Publish(channel, msg)

    let publishWithFlag channel msg flag (sub : ISubscriber) =
        sub.Publish(channel, msg, flag)

