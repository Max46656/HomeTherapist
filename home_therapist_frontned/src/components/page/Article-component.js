import React, { useState, useEffect } from "react";
import axios from "axios";
import { notification, Button, Table, Modal, Form, Input } from "antd";
import AuthService from "../../services/auth.service";
import { LayoutMarTop } from "../style";
import { useNavigate } from "react-router-dom";
import "../../css/styleTwo.css";


const UserArticles = () => {
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(false);
  const [visible, setVisible] = useState(false);
  const [form] = Form.useForm();
  const [currentArticle, setCurrentArticle] = useState(null);
 const navigate = useNavigate();
  useEffect(() => {
    fetchArticles();
  }, []);

  const fetchArticles = async () => {
    try {
      setLoading(true);
      const user = AuthService.getCurrentUser();
      const token = user.token;

      const headers = {
        Authorization: token,
      };

      const response = await axios.get("https://localhost:5000/api/Articles/ByUser", { headers });

      if (response.data.isSuccess) {
        setArticles(response.data.data);
      } else {
        notification.error({
          message: response.data.message,
          duration: 3,
        });
      }
    } catch (error) {
      console.error(error);
      notification.error({
        message: "Failed to fetch articles",
        duration: 3,
      });
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (record) => {
    form.setFieldsValue(record);
    setCurrentArticle(record);
    setVisible(true);
  };

  const handleDelete = async (id) => {
    try {
      setLoading(true);
      const user = AuthService.getCurrentUser();
      const token = user.token;

      const headers = {
        Authorization: token,
      };

      const response = await axios.delete(`https://localhost:5000/api/Articles/${id}`, { headers });

      if (response.data.isSuccess) {
        fetchArticles();
      } else {
        notification.error({
          message: response.data.message,
          duration: 3,
        });
      }
    } catch (error) {
      console.error(error);
      notification.error({
        message: "Failed to delete article",
        duration: 3,
      });
    } finally {
      setLoading(false);
    }
  };

  const handleUpdate = async (values) => {
    try {
      setLoading(true);
      const user = AuthService.getCurrentUser();
      const token = user.token;

      const headers = {
        Authorization: token,
      };

      const response = await axios.put(`https://localhost:5000/api/Articles/${currentArticle.id}`, values, { headers });

      if (response.data.isSuccess) {
        fetchArticles();
      } else {
        notification.error({
          message: response.data.message,
          duration: 3,
        });
      }
    } catch (error) {
      console.error(error);
      notification.error({
        message: "Failed to update article",
        duration: 3,
      });
    } finally {
      setLoading(false);
      setVisible(false);
      form.resetFields();
    }
  };

  const columns = [
    {
      title: "Title",
      dataIndex: "title",
      key: "title",
    },
    {
      title: "Subtitle",
      dataIndex: "subtitle",
      key: "subtitle",
    },
    {
      title: "Actions",
      dataIndex: "action",
      render: (_, record) => (
        <div>
          <Button onClick={() => handleEdit(record)}>Edit</Button>
          <Button onClick={() => handleDelete(record.id)}>Delete</Button>
        </div>
      ),
    },
  ];

  return (
    <div>
      <LayoutMarTop />
      <div className="container py-md-5 my-md-3 UserArticles_sm vh-100">

      <button className="btn color_2 mb-md-3 UserArticles_sm_style"  onClick={() => navigate("/:newArticle")}>新增文章</button>
      <Table
        dataSource={articles}
        columns={columns}
        rowKey="id"
        loading={loading}
      />
      <Modal
        title="Edit Article"
        visible={visible}
        onCancel={() => setVisible(false)}
        onOk={() => {
          form
            .validateFields()
            .then(values => {
              form.resetFields();
              handleUpdate(values);
            })
            .catch(info => {
              console.log('Validate Failed:', info);
            });
        }}
      >
        <Form
          form={form}
          layout="vertical"
          name="form_in_modal"
          initialValues={{ modifier: 'public' }}
        >
          <Form.Item
            name="title"
            label="Title"
            rules={[
              {
                required: true,
                message: 'Please input the title of the article!',
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="subtitle"
            label="Subtitle"
          >
            <Input type="textarea" />
          </Form.Item>
          <Form.Item name="body" label="Body">
            <Input.TextArea />
          </Form.Item>
        </Form>
      </Modal>
    </div>
        </div>
  );
};

export default UserArticles;
