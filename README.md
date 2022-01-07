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
* Command to run API (from the whatif.api folder):
    ```dapr run --app-id whatifapi --app-port 5178 --dapr-http-port 3602 --dapr-grpc-port 60002 --log-level Debug --components-path C:\projects\what-if\source\whatif.api\dapr\components dotnet run```
    replase --components-path with your path
* Command to run frontend (from the whatif.web folder):
    ```yarn start``` or ```npm run start```

## Testing endpoints
* Accessing swagger:
    ```http://localhost:5178/swagger/index.html```
* Rest client in VsCode
    - check file api.Rest in the api project
