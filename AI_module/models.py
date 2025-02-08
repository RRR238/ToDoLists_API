from pydantic import BaseModel
from typing import List


class To_do_item(BaseModel):
    title:str
    text:str
    deadline:str
    flag:str


class To_do_list(BaseModel):
    to_do_list_name: str
    to_do_list: List[To_do_item]
    owners: List[str]


class Similarity_search(BaseModel):
    limit: int
    text: str
    owner: str


class Item_to_add(BaseModel):
    to_do_list_name: str
    owners: List[str]
    text: str
    item_number: int