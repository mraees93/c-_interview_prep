# ☁️ Cloud & Deployment Translation Cheatsheet
*LexisNexis Interview Preparation - Infrastructure recall module*

---

## 🎭 The Real-World Analogy: Shipping Standard Containers

To explain cloud migrations and zero-downtime deployment concepts smoothly to a panel, use a commercial shipping yard analogy:

*   **The Docker Container:** A standardized steel shipping cargo crate. It does not care if it is stacked onto a Dutch cargo ship (**Azure**) or an American cargo ship (**AWS**); as long as the dimensions match, the crane can lift and run it identically.
*   **Blue/Green Deployment:** Running two identical shipping docks side-by-side. Dock A (Blue) is actively unloading cargo to the trucks. You build and load your new shipment onto Dock B (Green) in complete isolation. Once verified safe, you open the highway gates to route all incoming trucks to Dock B instantly. If a crate breaks on Dock B, you switch the highway gates back to Dock A immediately.

---

## 🔄 1. The Azure-to-AWS Concept Translation Matrix

The core physics of hosting and compute infrastructure are completely cloud-agnostic. Use this mapping to translate your Azure experience into AWS language:

| Architectural Tier | Your Azure Experience | LexisNexis AWS Equivalent |
| :--- | :--- | :--- |
| **Container Orchestration** | **AKS** *(Azure Kubernetes Service)* | **EKS** *(Elastic Kubernetes Service)* |
| **Serverless Containers** | **Azure Container Apps** | **AWS Fargate** *(Serverless ECS/EKS compute)* |
| **Object Data Storage** | **Azure Blob Storage** | **AWS S3** *(Simple Storage Service)* |
| **Private Isolated Network**| **Azure VNet** *(Virtual Network)* | **AWS VPC** *(Virtual Private Cloud)* |
| **Serverless Compute** | **Azure Functions** | **AWS Lambda** |
| **Container Image Registry**| **ACR** *(Azure Container Registry)* | **AWS ECR** *(Elastic Container Registry)* |

---

## 🛡️ 2. Core High-Availability Deployment Patterns

Panels expect intermediate engineers to protect live data production systems using automated, zero-downtime rollout boundaries:

*   **Blue/Green Deployments:** Running two identical production environments. Only one ("Blue") serves live customer traffic. The new compiled C# container version is deployed to the idle staging environment ("Green"). After automated infrastructure health checks pass, traffic is seamlessly routed to Green at the load-balancer layer. If an error spike occurs, traffic routes back to Blue instantly with zero user disruption.
*   **Canary Deployments:** Shifting traffic incrementally. You route a tiny fraction (e.g., 5% of web requests) to the new container cluster. You then actively monitor performance metrics and logs. If no telemetry triggers trip over a designated cooling window, the remaining traffic is incrementally migrated until the old containers are safely decommissioned.
*   **Infrastructure as Code (IaC):** Managing systems declaratively. While Azure relies on ARM Templates or Bicep, AWS natively utilizes **CloudFormation**. However, production systems frequently leverage **Terraform** to remain agnostic. 

---

## 👑 3. Your Go-To Cloud Transition Defense Script

If a panel interviewer asks about your lack of direct AWS exposure, deliver this response:

> *"While my primary hands-on production experience has been anchored to the Azure ecosystem—such as building pipelines for Azure Kubernetes Service and configuring automated GitHub Actions rollouts—the fundamental physics of cloud deployment are completely transferable. Regardless of whether a cluster is running on Azure AKS or AWS EKS, the architectural goals remain identical: we are containerizing our .NET core services inside lightweight Docker envelopes, managing storage volumes securely, and enforcing automated zero-downtime Blue/Green gates to isolate production blast radiuses."*
