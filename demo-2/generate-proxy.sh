protoc -I/usr/local/include -I. \
  -I$GOPATH/src \
  -I$GOPATH/src/github.com/grpc-ecosystem/grpc-gateway/third_party/googleapis \
  --go_out=plugins=grpc:components/order-proxy/moonbucks/ \
  protos/moonbucks.proto
  
  protoc -I/usr/local/include -I. \
  -I$GOPATH/src \
  -I$GOPATH/src/github.com/grpc-ecosystem/grpc-gateway/third_party/googleapis \
  --grpc-gateway_out=logtostderr=true:components/order-proxy/moonbucks/ \
  protos/moonbucks.proto

  protoc -I/usr/local/include -I. \
  -I$GOPATH/src \
  -I$GOPATH/src/github.com/grpc-ecosystem/grpc-gateway/third_party/googleapis \
  --swagger_out=logtostderr=true:components/order-proxy/ \
  ./protos/moonbucks.proto

cp ./components/order-proxy/moonbucks/protos/* ./components/order-proxy/moonbucks/ 
cp ./components/order-proxy/protos/* ./components/order-proxy/ 
rm -r ./components/order-proxy/moonbucks/protos
rm -r ./components/order-proxy/protos

# https://grpc-ecosystem.github.io/grpc-gateway/docs/grpcapiconfiguration.html