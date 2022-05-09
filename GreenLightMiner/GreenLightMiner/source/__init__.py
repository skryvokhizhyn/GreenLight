from source.sourceaws import SourceAws
from source.sourcedb import SourceDb

def get_source(argv):
    if argv[2] == "db":
        if (len(argv) < 4):
            raise Exception("no path to DB specified")

        return SourceDb(argv[3])
    elif argv[2] == "aws":
        return SourceAws()
    else:
        raise Exception("Unexpected source type. Valid options are [db, aws]")