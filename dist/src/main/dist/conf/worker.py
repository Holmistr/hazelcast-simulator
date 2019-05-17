import sys

from pyworker.worker import Worker

#sys.stdout = open("worker.out", "w")
#sys.stderr = open("worker.err", "w")

worker = Worker()
worker.start()

#sys.stdout.close()
#sys.stderr.close()