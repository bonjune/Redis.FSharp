namespace Redis.FSharp

open StackExchange.Redis

type RedisConnectionBuilder =
    { ConnectionString : string }

[<RequireQualifiedAccess>]
module Redis =
    (* Connections *)
    let init =
        { ConnectionString = "" }
    
    /// https://stackexchange.github.io/StackExchange.Redis/Configuration
    let options configString =
        ConfigurationOptions.Parse configString
    
    let connString connString builder =
        { builder with ConnectionString = connString }

    /// Caution: do not make multiple multiplexers! Just re-use it!
    let connect () builder =
        ConnectionMultiplexer.Connect builder.ConnectionString
    
    let server host port (multiplexer : ConnectionMultiplexer) =
        multiplexer.GetServer(host, port)
    
    let endpoints (multiplexer : ConnectionMultiplexer) =
        multiplexer.GetEndPoints()
    
    let clients (server : IServer) =
        server.ClientList()


    (* Database *)
    let db (multiplexer : ConnectionMultiplexer) =
        multiplexer.GetDatabase()
    
    let stringSet (key: RedisKey) (value: RedisValue) (db : IDatabase) =
        db.StringSet(key, value)
    
    let stringSetAsync (key: RedisKey) (value: RedisValue) (db : IDatabase) =
        db.StringSetAsync(key, value)

    let stringGet (key : RedisKey) (db : IDatabase) =
        db.StringGet key
    
    let stringGetAsync (key : RedisKey) (db : IDatabase) =
        db.StringGetAsync key

    let stringGetWithFlag (key : RedisKey) flag (db : IDatabase) =
        db.StringGet(key, flag)

    (* Pub/Sub *)
    let subscriber (multiplexer : ConnectionMultiplexer) =
        multiplexer.GetSubscriber()
    
    let subscribe channel observable (sub : ISubscriber) =
        sub.Subscribe(channel, observable)
    
    let publish channel msg (sub : ISubscriber) =
        sub.Publish(channel, msg)

    let publishWithFlag channel msg flag (sub : ISubscriber) =
        sub.Publish(channel, msg, flag)

