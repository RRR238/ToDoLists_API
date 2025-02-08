import os
from dotenv import load_dotenv

load_dotenv()

class Vector_search_config:

        embedding_model = os.getenv("EMBEDDING_MODEL")
        vector_size = os.getenv("VECTOR_SIZE")
        collection = os.getenv("COLLECTION")
        host = os.getenv("QDRANT_HOST")