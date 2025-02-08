from services import Qdrant_repository, AsyncQdrantClient
import asyncio
from vector_search_config import Vector_search_config


if __name__ == "__main__":
    qdrant_repo = Qdrant_repository(AsyncQdrantClient(Vector_search_config.host),Vector_search_config.collection)
    # Run the create collection function in an asyncio loop
    asyncio.run(qdrant_repo.create_collection())