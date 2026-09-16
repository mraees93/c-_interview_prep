## ✉️ RabbitMQ Worker Cloud Scaling Physics

### 1. The Code Constraint
* **The Target:** `_channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);`
* **The Rule:** Enforces a strict **Pull-on-Demand** limit. One worker container pulls exactly **1 unacknowledged message** from the queue, blocking message hoarding.

### 2. The Docker Container Layer
* **The Reality:** Your `BackgroundService` queue and worker code are packaged into a single **Docker Container Image**. This image serves as the static blueprint that the cloud multiplies.

### 3. Vertical Scaling (Size Tuning)
* **The Reality:** Adjusting the static resource limits (e.g., 0.5 vCPU, 1GB RAM) allocated to a running container instance. **This is configured in the cloud deployment manifest (Azure YAML), not the Dockerfile.** It does not clone workers or provide high availability.

### 4. Horizontal Scaling (Dynamic Cloning)
* **The Reality:** Handled entirely by Azure monitoring the queue depth. The cloud starts with **2 container workers by default** for High Availability. During a sudden traffic spike, Azure dynamically **clones that Docker image up to 10 or 20 competing worker containers** to smash the workload.
