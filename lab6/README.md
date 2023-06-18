# Zookeeper Watcher

This is a Python-based application that interacts with a Zookeeper instance. When a specific znode is created, it opens an application. When the znode is deleted, it closes the application. The application name is passed as a command-line argument.

## Dependencies

- Docker
- Python 3.7 or higher
- Kazoo Python library

## Setup

Clone the repository:

```bash
git clone https://github.com/Seszele/zookeeper-watcher.git
cd zookeeper-watcher
```

Install the dependencies:

```bash
pip install -r requirements.txt
```

## Running Zookeeper with Docker Compose
We will run a Zookeeper instance using Docker Compose.
```bash
docker-compose up -d
```
## Usage
Run the Python script:
```bash
python client.py <app_name>
```
Replace `<app_name>` with the name of the application you want to open and close.

## Commands
Once the script is running, you can interact with it:
* To create a znode, type: `create /path/to/znode`
* To delete a znode, type: `delete /path/to/znode`
* To display the current znode tree, type: `display`
* To exit the script, type: `exit`