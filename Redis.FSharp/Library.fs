namespace Redis.FSharp

open StackExchange.Redis
open FSharp.Control.Tasks.V2.ContextInsensitive

type RedisConnectionBuilder = { ConnectionString : string }

module RedisValue =
    let toOption (value : RedisValue) =
        if value.IsNullOrEmpty then None else Some value

[<RequireQualifiedAccess>]
module Redis =
    (* Connections *)
    let init = { ConnectionString = "" }

    /// https://stackexchange.github.io/StackExchange.Redis/Configuration
    let configOptsFromString configString =
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

    let clients (server : IServer) = server.ClientList()

    (* Database *)

    let db (multiplexer : ConnectionMultiplexer) =
        multiplexer.GetDatabase()

    let stringSet (key : RedisKey) (value : RedisValue) (db : IDatabase) =
        db.StringSet(key, value)

    let stringSetAsync (key : RedisKey) (value : RedisValue) (db : IDatabase) =
        db.StringSetAsync(key, value)

    let stringGet (key : RedisKey) (db : IDatabase) =
        db.StringGet key |> RedisValue.toOption

    let stringGetAsync (key : RedisKey) (db : IDatabase) =
        task {
            let! value = db.StringGetAsync key
            return RedisValue.toOption value
        }

    let stringGetWithFlag (key : RedisKey) flag (db : IDatabase) =
        db.StringGet(key, flag) |> RedisValue.toOption

    let keyDelete (key : RedisKey) (db : IDatabase) = db.KeyDelete(key)

    let keyDeleteWithFlag (key : RedisKey) flag (db : IDatabase) =
        db.KeyDelete(key, flag)

    let keyDeleteMany (keys : RedisKey []) (db : IDatabase) = db.KeyDelete(keys)

    let keyDeleteManyWithFlag (keys : RedisKey []) flag (db : IDatabase) =
        db.KeyDelete(keys, flag)

    let keyDeleteAsync (key : RedisKey) (db : IDatabase) =
        db.KeyDeleteAsync(key)

    let keyDeleteWithFlagAsync (key : RedisKey) flag (db : IDatabase) =
        db.KeyDeleteAsync(key, flag)

    let keyDeleteManyAsync (keys : RedisKey []) (db : IDatabase) =
        db.KeyDeleteAsync(keys)

    let keyDeleteManyWithFlagAsync (keys : RedisKey []) flag (db : IDatabase) =
        db.KeyDeleteAsync(keys, flag)

    (* Pub/Sub *)

    let subscriber (multiplexer : ConnectionMultiplexer) =
        multiplexer.GetSubscriber()

    let subscribe channel (continuation : RedisChannel -> RedisValue -> unit) (sub : ISubscriber) =
        sub.Subscribe(channel, continuation)

    let subscribeWithFlag channel continuation flag (sub : ISubscriber) =
        sub.Subscribe(channel, continuation, flag)

    let publish channel msg (sub : ISubscriber) =
        sub.Publish(channel, msg)

    let publishWithFlag channel msg flag (sub : ISubscriber) =
        sub.Publish(channel, msg, flag)
