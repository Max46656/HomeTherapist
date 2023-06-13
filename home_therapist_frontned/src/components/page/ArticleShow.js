import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import { LayoutMarTop } from "../style";
import "../../css/styleTwo.css";

const ArticleShow = () => {
  const { id } = useParams();
  const [article, setArticle] = useState(null);

  useEffect(() => {
    axios
      .get(`https://localhost:5000/api/Articles/${id}`)
      .then((res) => {
        console.log(res);
        const data = res.data.data;
        setArticle(data);
      })
      .catch((err) => console.log(err));
  }, [id]);

  if (!article) {
    return null;
  }

  return (
    <div>
      <br />
      <LayoutMarTop />
       <div className="container py-md-5 articleShow_sm">
       <div className="row articleShow_sm_style">
       <div className="col-tt-11">
        <h2>{article.title}</h2>
        {article.subtitle && <h3>{article.subtitle}</h3>}
        <div dangerouslySetInnerHTML={{ __html: article.body}} />

       </div>
      </div>

       </div>
    </div>
  );
};

export default ArticleShow;
