from sentence_transformers import SentenceTransformer
from qdrant_client import AsyncQdrantClient, models
from qdrant_client.models import Filter, FieldCondition, MatchValue
import uuid
from vector_search_config import Vector_search_config


class Embedding_service:
    def __init__(self,
                 model_name: str = Vector_search_config.embedding_model):
        self.model = SentenceTransformer(model_name)

    def get_embedding(self, text: str):
        """Generate embedding for a given text"""
        return self.model.encode(text).tolist()


class Qdrant_repository:

    def __init__(self, client,
                 collection,
                 embedding_size = Vector_search_config.vector_size):
        self.client = client
        self.embedding = Embedding_service()
        self.collection = collection
        self.embedding_size = embedding_size

    async def create_collection(self):
        await self.client.create_collection(
            collection_name=self.collection,
            vectors_config=models.VectorParams(size=int(self.embedding_size),
                                            distance=models.Distance.COSINE),
        )

        print("collection created")
        await self.client.close()
        print("connection closed")

    async def insert_vector(self, text, metadata):
        point_id = str(uuid.uuid4())
        embedded_text = self.embedding.get_embedding(text)

        await self.client.upsert(
            collection_name=self.collection,
            points=[
                models.PointStruct(
                    id=point_id,
                    payload=metadata,
                    vector=embedded_text,
                ),
            ],
        )

        return "ok"

    async def query(self, text, owner_name, limit):
        embedded_text = self.embedding.get_embedding(text)

        results = await self.client.query_points(
            collection_name=self.collection,
            query=embedded_text,
            limit=limit,
            query_filter=Filter(
                must=[
                    FieldCondition(
                        key="owners",
                        match=MatchValue(value=owner_name)  # Ensure owner_name is in owners list
                    )
                ]
            )
        )

        return results

    async def delete_vectors(self, to_do_list_name: str):
        """Deletes all vectors where 'to_do_list_name' matches the given key"""
        await self.client.delete(
            collection_name=self.collection,
            points_selector=models.Filter(
                must=[
                    models.FieldCondition(
                        key="to_do_list_name",
                        match=models.MatchValue(value=to_do_list_name)  # Exact match condition
                    )
                ]
            ),
        )
        return "Deleted"


async def get_qdrant_client(host = Vector_search_config.host) -> AsyncQdrantClient:
    """ Dependency to provide a Qdrant client, which is closed after the request. """
    client = AsyncQdrantClient(url=f"http://{host}:6333")
    try:
        yield client  # Yield the client to be used in the route
    finally:
        # Ensuring the client is closed after the request is finished
        await client.close()
        print("Qdrant client closed after the request.")
