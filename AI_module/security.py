import jwt
from fastapi.security import OAuth2PasswordBearer
from fastapi import Depends, HTTPException
from dotenv import load_dotenv
import os
load_dotenv()


class Security_config:

    SECRET_KEY = os.getenv("SECRET_KEY")
    ALGORITHM = os.getenv("ALGORITHM")
    ISSUER = os.getenv("ISSUER")
    AUDIENCE = os.getenv("AUDIENCE")


oauth2Scheme = OAuth2PasswordBearer(tokenUrl="login")


def endpoint_verification(token: str = Depends(oauth2Scheme)):

    try:
        # Decode the JWT token
        payload = jwt.PyJWT().decode(token, Security_config.SECRET_KEY, algorithms=[Security_config.ALGORITHM],
                             audience=Security_config.AUDIENCE, issuer = Security_config.ISSUER)
        return payload
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token has expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")
    except jwt.PyJWTError:
        raise HTTPException(status_code=401, detail="Error decoding token")
