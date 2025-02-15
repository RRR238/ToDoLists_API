import os
from fastapi import FastAPI, Depends, status
from fastapi.responses import JSONResponse
from models import To_do_list, Similarity_search, Item_to_add
import uvicorn
from security import endpoint_verification
from services import AsyncQdrantClient, get_qdrant_client
from services import Qdrant_repository
from vector_search_config import Vector_search_config

app =FastAPI()


@app.post("/insert_items", status_code=status.HTTP_201_CREATED)
async def test(to_do_list:To_do_list,
         decoded_token = Depends(endpoint_verification),
         client: AsyncQdrantClient = Depends(get_qdrant_client)):

    qdrant_repo = Qdrant_repository(client, Vector_search_config.collection)
    item_number = 1
    for i in to_do_list.to_do_list:
        metadata = {"item_number":item_number,
                    "to_do_list_name":to_do_list.to_do_list_name,
                    "owners":to_do_list.owners}
        await qdrant_repo.insert_vector(i.text, metadata)
        item_number += 1

    return {"status": "Created"}


@app.post("/search_items")
async def insert_vectors(similarity_payload: Similarity_search,
                        decoded_token = Depends(endpoint_verification),
                         client: AsyncQdrantClient = Depends(get_qdrant_client)):

    qdrant_repo = Qdrant_repository(client, Vector_search_config.collection)
    results = await qdrant_repo.query(similarity_payload.text,
                                        similarity_payload.owner,
                                        similarity_payload.limit)

    filtered_results = [{"to_do_list_name":point.payload["to_do_list_name"],
                         "item_number":point.payload["item_number"]} for point in results.points]

    return JSONResponse(status_code=200, content={"responseItems":filtered_results})


@app.post("/add_item", status_code=status.HTTP_201_CREATED)
async def add_item(item_to_add:Item_to_add,
                        decoded_token = Depends(endpoint_verification),
                        client: AsyncQdrantClient = Depends(get_qdrant_client)):

    qdrant_repo = Qdrant_repository(client, Vector_search_config.collection)
    metadata = {"item_number": item_to_add.item_number,
                "to_do_list_name": item_to_add.to_do_list_name,
                "owners": item_to_add.owners}

    await qdrant_repo.insert_vector(item_to_add.text, metadata)
    return {"status": "Created"}


@app.delete("/delete_items/{list_name}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_items(list_name:str,
                       decoded_token=Depends(endpoint_verification),
                       client: AsyncQdrantClient = Depends(get_qdrant_client)
                       ):

    qdrant_repo = Qdrant_repository(client, Vector_search_config.collection)
    result = await qdrant_repo.delete_vectors(list_name)


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=int(os.getenv("LOCAL_PORT")))