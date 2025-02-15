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
        print(token)
        print(Security_config.SECRET_KEY)
        print(Security_config.ALGORITHM)
        print(Security_config.AUDIENCE)
        print(Security_config.ISSUER)
        payload = jwt.decode(token, options={"verify_signature": False})
        print("Decoded payload without verification:", payload)
        payload = jwt.PyJWT().decode(token, Security_config.SECRET_KEY, algorithms=[Security_config.ALGORITHM],
                             audience=Security_config.AUDIENCE, issuer = Security_config.ISSUER)
        print(payload)
        return payload
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token has expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")
    except jwt.PyJWTError:
        raise HTTPException(status_code=401, detail="Error decoding token")
    except Exception as e:
        print(e)

try:
    payload = jwt.PyJWT().decode("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJpbnRlcm5hbC1zZXJ2aWNlIiwiZXhwIjoxNzM5NjIzMDE1LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjcxMTgiLCJhdWQiOiJUb0RvTGlzdHNBUEkifQ.bKHi8nAc2EFEryeQR4IaqM8rOoR46n2p4vRRYW_Hv6A", Security_config.SECRET_KEY, algorithms=[Security_config.ALGORITHM],
                             audience=Security_config.AUDIENCE, issuer = Security_config.ISSUER)
except Exception as e:
    print(e)