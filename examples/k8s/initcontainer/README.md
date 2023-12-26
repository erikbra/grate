## Grate with k8s

You can propably run grate on your production environment using k8s init container. Please see the very basic example how to config and deploy to k8s.

## Prerequisite: 

Local k8s simulator: you can use [minikube](https://github.com/kubernetes/minikube) (my favorite) or [kind](https://github.com/kubernetes-sigs/kind)

## Usage

Now let's get started with your terminal (any Linux dist, MacOS or WSL2):
 - Open your terminal and start minikube

```sh
  minikube start
```

 - Apply the deployment 
  
```sh
  kubectl apply -f deployment.yaml 
```
  - You can check the status with command
  ```sh 
  kubectl get pods -w  | grep grate
  ```
  - After the pod started, let's test the data :D

```sh
  kubectl port-forward svc/grate-k8s 5000:5000
  # sending the http request
  curl -sL http://localhost:5000/api/grate | jq
```
- Done. Remember to destroy the cluster
```sh
  minikube stop
```

## Notes

- Curious how it works, see the `Dockerfile` and `Dockerfile-db`.

