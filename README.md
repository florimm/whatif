# whatIf

_whatIf_ is a small application to track your cryptocurrencies wallet.

The app features:
* Real-time track of your wallet.
* Horizontal scaling.

The goals of this app are:
1. Showing how to use Actors, State, PubSub in Dapr.
1. Presenting a semi-real-world use case of the distributed actor model.
1. Learning how to use Dapr (for myself).

**[Find more about Dapr here.](https://dapr.io/)**


## Running the app
* To run API, you need to have dapr cli and tye (https://github.com/dotnet/tye) installed:
    ```tye run```
    
* To run frontend, you must install packages with command ```yarn``` or ```npm i``` (from the whatif.web folder) then to start you run ```yarn start``` or ```npm run start```

## Testing endpoints
* Rest client in VsCode
    - check file api.Rest in the api project and change the ports to the one that the dapr sidecar has started with
