import jwt
from fastapi.security import OAuth2PasswordBearer
from fastapi import Depends, HTTPException
from dotenv import load_dotenv
import os
load_dotenv()


class Security_config:
    def __init__(self):
        self.SECRET_KEY = os.getenv("SECRET_KEY")
        self.ALGORITHM = os.getenv("ALGORITHM")
        self.ISSUER = os.getenv("ISSUER")
        self.AUDIENCE = os.getenv("AUDIENCE")


oauth2Scheme = OAuth2PasswordBearer(tokenUrl="login")


def endpoint_verification(token: str = Depends(oauth2Scheme)):

    sec_config = Security_config()
    try:
        # Decode the JWT token
        payload = jwt.decode(token, sec_config.SECRET_KEY, algorithms=[sec_config.ALGORITHM],
                             audience=sec_config.AUDIENCE, issuer = sec_config.ISSUER)
        return payload
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token has expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")
    except jwt.PyJWTError:
        raise HTTPException(status_code=401, detail="Error decoding token")

