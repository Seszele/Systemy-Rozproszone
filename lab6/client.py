import signal
from kazoo.client import KazooClient
from kazoo.client import KazooState
from kazoo.recipe.watchers import DataWatch, ChildrenWatch
from subprocess import Popen
import sys


class ZookeeperApp:
    def __init__(self, hosts):
        self.zk = KazooClient(hosts=hosts)
        self.zk.add_listener(self.my_listener)
        self.process = None
        self.previous_children = []
        self.application_name = "mspaint"
        if len(sys.argv) > 1 and sys.argv[1] != None:
            self.application_name = sys.argv[1]
        else:
            print("No application name provided, using default: mspaint")

    def my_listener(self, state):
        if state == KazooState.LOST:
            print("Session lost")
        elif state == KazooState.SUSPENDED:
            print("Session suspended")
        else:
            print("Connected or reconnected")

    def start(self):
        self.zk.start()

        if self.zk.exists("/z"):
            print("Znode '/z' already exists")
        else:
            print("Znode '/z' does not exist yet")

        @DataWatch(self.zk, "/z")
        def watch_znode(data, stat, event):
            if event is not None:
                if event.type == "CREATED":
                    print("Znode '/z' has been created")
                    # Open appliation
                    self.process = Popen([self.application_name])

                    self.add_children_watch()

                elif event.type == "DELETED":
                    print("Znode '/z' has been deleted")
                    # Close appliation
                    if self.process:
                        self.process.send_signal(signal.CTRL_C_EVENT)
                        self.process.terminate()
                        self.process = None
                elif event.type == "CHANGED":
                    print("Znode '/z' has changed")

        self.add_children_watch()

    def display_tree(self, node_path="/z", level=0):
        if not self.zk.exists(node_path):
            print("Empty")
            return
        children = self.zk.get_children(node_path)
        print("    " * level + node_path)
        for child in children:
            self.display_tree(f"{node_path}/{child}", level + 1)

    def add_children_watch(self, node_path="/z"):
        @ChildrenWatch(self.zk, node_path)
        def watch_children(children):
            if len(children) > len(self.previous_children):
                print("Number of children to '/z': ", len(children))
            self.previous_children = children

    def create_znode(self, node_path):
        if not self.zk.exists(node_path):
            self.zk.create(node_path, ephemeral=False, makepath=True)

    def delete_znode(self, node_path):
        self.zk.delete(node_path, recursive=True)

    def stop(self):
        self.zk.stop()


if __name__ == "__main__":
    app = ZookeeperApp("localhost:2181")
    try:
        app.start()
        while True:
            command = input(">")
            if "create" in command:
                _, node_path = command.split(" ")
                app.create_znode(node_path)
            elif "delete" in command:
                _, node_path = command.split(" ")
                app.delete_znode(node_path)
            elif "display" in command:
                app.display_tree()
            elif command.lower() == "exit":
                break
    finally:
        app.stop()
