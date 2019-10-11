
1. Follow the steps from https://grpc-ecosystem.github.io/grpc-gateway/docs/usage.html
    


curl -X POST "http://localhost:9010/v1/orders" -H "accept: application/json" -H "Content-Type: application/json" -d "{ \"size\": \"SMALL\", \"coffee_type\": \"string\"}"